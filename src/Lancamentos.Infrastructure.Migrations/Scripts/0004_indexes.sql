CREATE INDEX IF NOT EXISTS ix_lancamentos_data           ON lancamentos (data);
CREATE INDEX IF NOT EXISTS ix_lancamentos_tipo            ON lancamentos (tipo);
CREATE INDEX IF NOT EXISTS ix_outbox_messages_processado  ON outbox_messages (processado_em) WHERE processado_em IS NULL;
