CREATE TABLE IF NOT EXISTS lancamentos (
    id          UUID            NOT NULL DEFAULT gen_random_uuid(),
    data        DATE            NOT NULL,
    tipo        INTEGER         NOT NULL,
    valor       NUMERIC(18, 2)  NOT NULL,
    descricao   VARCHAR(500)    NOT NULL,

    CONSTRAINT pk_lancamentos PRIMARY KEY (id)
);
