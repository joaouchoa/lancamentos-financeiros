CREATE TABLE IF NOT EXISTS lancamentos_processados (
    lancamento_id   UUID            NOT NULL,
    processado_em   TIMESTAMPTZ     NOT NULL DEFAULT now(),

    CONSTRAINT pk_lancamentos_processados PRIMARY KEY (lancamento_id)
);
