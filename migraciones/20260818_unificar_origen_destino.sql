-- VoyLlegando: unificacion de Productores/Campos con Plantas/Destinos
-- Ejecutar una sola vez y solamente con public.viajes vacia.

BEGIN;

DO $$
BEGIN
    IF EXISTS (SELECT 1 FROM public.viajes LIMIT 1) THEN
        RAISE EXCEPTION
            'La tabla viajes debe estar vacia antes de unificar origen y destino.';
    END IF;
END
$$;

DO $$
DECLARE
    restriccion record;
BEGIN
    FOR restriccion IN
        SELECT conname
        FROM pg_constraint
        WHERE conrelid = 'public.viajes'::regclass
          AND contype = 'f'
          AND pg_get_constraintdef(oid) ~
              'FOREIGN KEY \((id_produc|id_origen|id_planta|id_destino)\)'
    LOOP
        EXECUTE format(
            'ALTER TABLE public.viajes DROP CONSTRAINT %I',
            restriccion.conname
        );
    END LOOP;
END
$$;

ALTER TABLE public.viajes
    RENAME COLUMN id_produc TO id_planta_origen;

ALTER TABLE public.viajes
    RENAME COLUMN id_origen TO id_destino_origen;

ALTER TABLE public.viajes
    RENAME COLUMN id_planta TO id_planta_destino;

ALTER TABLE public.viajes
    RENAME COLUMN id_destino TO id_destino_destino;

ALTER TABLE public.viajes
    ADD CONSTRAINT viajes_origen_fkey
        FOREIGN KEY (id_destino_origen, id_planta_origen)
        REFERENCES public.destinos(id_destino, id_planta),
    ADD CONSTRAINT viajes_destino_fkey
        FOREIGN KEY (id_destino_destino, id_planta_destino)
        REFERENCES public.destinos(id_destino, id_planta);

DROP VIEW public.vw_viajes_detalle;

CREATE VIEW public.vw_viajes_detalle AS
SELECT
    x.id_viaje,
    x.id_transpor,
    l.nombre AS logistica,
    x.id_camionero,
    m.nombre AS nombempre,
    x.id_planta_origen AS id_produc,
    po.nombre AS productor,
    x.id_destino_origen AS id_origen,
    dor.descrip_destino AS origen,
    x.id_planta_destino AS id_planta,
    pd.nombre AS planta,
    x.id_destino_destino AS id_destino,
    dde.descrip_destino AS destino,
    x.id_cereal,
    c.nombre_cereal AS cereal,
    x.fecha_pedido,
    x.ctg,
    x.kms,
    x.tarifa,
    x.estado,
    e.descrip_via,
    x.observaciones,
    x.batea,
    x.corta,
    x.larga,
    x.fecha_asigna,
    x.fecha_termina
FROM public.viajes x
JOIN public.cereales c ON c.id_cereal = x.id_cereal
JOIN public.logisticas l ON l.id_transpor = x.id_transpor
LEFT JOIN public.usuarios m ON m.id_usuario = x.id_camionero
JOIN public.plantas po ON po.id_planta = x.id_planta_origen
JOIN public.destinos dor ON dor.id_destino = x.id_destino_origen
JOIN public.plantas pd ON pd.id_planta = x.id_planta_destino
JOIN public.destinos dde ON dde.id_destino = x.id_destino_destino
JOIN public.estavia e ON e.estado::text = x.estado::text;

COMMIT;

-- Productores y campos se conservan hasta validar todo el flujo.
