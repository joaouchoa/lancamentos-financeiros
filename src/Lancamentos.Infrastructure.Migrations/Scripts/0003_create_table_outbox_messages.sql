CREATE TABLE IF NOT EXISTS outbox_messages (
    id              UUID            NOT NULL DEFAULT gen_random_uuid(),
    tipo            VARCHAR(200)    NOT NULL,
    payload         TEXT            NOT NULL,
    criado_em       TIMESTAMPTZ     NOT NULL DEFAULT now(),
    processado_em   TIMESTAMPTZ     NULL,

    CONSTRAINT pk_outbox_messages PRIMARY KEY (id)
);
