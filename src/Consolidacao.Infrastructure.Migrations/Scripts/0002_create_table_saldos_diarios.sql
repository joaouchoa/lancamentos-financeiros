CREATE TABLE IF NOT EXISTS saldos_diarios (
    id              UUID            NOT NULL DEFAULT gen_random_uuid(),
    data            DATE            NOT NULL,
    total_creditos  NUMERIC(18, 2)  NOT NULL DEFAULT 0,
    total_debitos   NUMERIC(18, 2)  NOT NULL DEFAULT 0,
    saldo           NUMERIC(18, 2)  NOT NULL DEFAULT 0,

    CONSTRAINT pk_saldos_diarios PRIMARY KEY (id),
    CONSTRAINT uq_saldos_diarios_data UNIQUE (data)
);
