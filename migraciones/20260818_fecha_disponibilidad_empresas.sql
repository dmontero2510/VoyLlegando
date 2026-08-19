BEGIN;

ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS fecha_disponibilidad timestamp with time zone;

UPDATE public.usuarios
SET fecha_disponibilidad = CURRENT_TIMESTAMP
WHERE rol = 'E'
  AND estado = 'D'
  AND fecha_disponibilidad IS NULL;

CREATE INDEX IF NOT EXISTS ix_usuarios_empresas_disponibles_fifo
    ON public.usuarios (fecha_disponibilidad, id_usuario)
    WHERE rol = 'E'
      AND estado = 'D'
      AND habilitado = TRUE;

COMMIT;
