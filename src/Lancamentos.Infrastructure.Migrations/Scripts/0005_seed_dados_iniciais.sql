-- Lançamentos de exemplo distribuídos em 3 dias (hoje, ontem e anteontem),
-- para permitir consultas iniciais sem precisar registrar nada manualmente.
--
-- Os mesmos LancamentoId são referenciados no seed do serviço de Consolidação
-- (tabela lancamentos_processados) e os totais batem exatamente com os saldos
-- diários semeados lá. A tabela outbox_messages também é preenchida aqui,
-- já marcada como publicada (processado_em preenchido), simulando que o
-- OutboxPublisher já drenou essas mensagens em tempo real, momentos depois de
-- cada lançamento ser registrado — coerente com o restante do fluxo simulado.
--
-- tipo: 0 = Credito, 1 = Debito (mesma ordem do enum TipoLancamento)

INSERT INTO lancamentos (id, data, tipo, valor, descricao)
SELECT
    id,
    (CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date,
    tipo,
    valor,
    descricao
FROM (VALUES
    ('81548384-4c74-444f-b1d9-09c470f01589'::uuid, 2, 0, 1200.00::numeric(18,2), 'Venda de mercadoria - Cliente A'),
    ('9e7bbb40-4eac-4dae-83aa-d22785c52479'::uuid, 2, 0, 350.50::numeric(18,2),  'Venda de mercadoria - Cliente B'),
    ('bd2778f7-580f-4bbe-a5a7-fb1b224ecd6d'::uuid, 2, 1, 400.00::numeric(18,2),  'Pagamento de fornecedor - Insumos'),

    ('806810d7-26fb-4e52-bcc7-b53041882d47'::uuid, 1, 0, 800.00::numeric(18,2),  'Venda de mercadoria - Cliente C'),
    ('adf7f494-4423-4cb6-a875-f5c349ec143c'::uuid, 1, 1, 250.75::numeric(18,2),  'Pagamento de aluguel'),
    ('f21cee3f-80ed-4c46-8782-954fd069501a'::uuid, 1, 1, 120.00::numeric(18,2),  'Compra de material de escritório'),

    ('d050f4f0-5cbd-45cb-b28d-6eae171340e7'::uuid, 0, 0, 2000.00::numeric(18,2), 'Venda de mercadoria - Cliente D'),
    ('18fb2bc5-1607-44c1-9bfd-98725f366748'::uuid, 0, 0, 150.00::numeric(18,2),  'Venda de mercadoria - Cliente E'),
    ('b0d73452-0d6a-44a5-bab1-f6ea8dddb8be'::uuid, 0, 1, 600.00::numeric(18,2),  'Pagamento de funcionários')
) AS t(id, dias_atras, tipo, valor, descricao)
ON CONFLICT (id) DO NOTHING;

-- Uma mensagem de outbox por lançamento acima, cada uma com seu próprio
-- horário de criação (mesmo dia do lançamento) e um pequeno atraso até a
-- publicação, simulando o ciclo de polling do OutboxPublisher (~5s).
INSERT INTO outbox_messages (id, tipo, payload, criado_em, processado_em)
SELECT
    outbox_id,
    'LancamentoRegistradoIntegrationEvent',
    '{"LancamentoId":"' || lancamento_id || '","Data":"' ||
        to_char((CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date, 'YYYY-MM-DD') ||
        '","Tipo":"' || tipo_lancamento || '","Valor":' || valor || '}',
    (CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date + hora,
    (CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date + hora + (atraso_publicacao_segundos || ' seconds')::interval
FROM (VALUES
    ('a43fbf7b-1f12-4d92-9157-ee446cd02e5a'::uuid, '81548384-4c74-444f-b1d9-09c470f01589'::uuid, 2, TIME '09:14:22', 'Credito', 1200.00::numeric(18,2), 5),
    ('7bc4f4c1-ecae-455e-b163-d63006624928'::uuid, '9e7bbb40-4eac-4dae-83aa-d22785c52479'::uuid, 2, TIME '11:38:07', 'Credito', 350.50::numeric(18,2),  6),
    ('b1fc734d-ba24-4113-815e-11053c93f451'::uuid, 'bd2778f7-580f-4bbe-a5a7-fb1b224ecd6d'::uuid, 2, TIME '15:52:41', 'Debito',  400.00::numeric(18,2),  4),

    ('0410d650-1e50-49e5-b8d6-bd4fce1bb888'::uuid, '806810d7-26fb-4e52-bcc7-b53041882d47'::uuid, 1, TIME '08:47:15', 'Credito', 800.00::numeric(18,2),  7),
    ('30281d1f-3559-4a44-a269-5220dc8423e9'::uuid, 'adf7f494-4423-4cb6-a875-f5c349ec143c'::uuid, 1, TIME '13:05:38', 'Debito',  250.75::numeric(18,2),  5),
    ('d5ca832e-2416-4f97-8a36-83a2b59df7a4'::uuid, 'f21cee3f-80ed-4c46-8782-954fd069501a'::uuid, 1, TIME '17:22:59', 'Debito',  120.00::numeric(18,2),  6),

    ('ca6b9fe3-fbae-4c12-b29f-a88a0baa385a'::uuid, 'd050f4f0-5cbd-45cb-b28d-6eae171340e7'::uuid, 0, TIME '09:30:12', 'Credito', 2000.00::numeric(18,2), 5),
    ('77af3f0c-43cc-4071-bf76-b1bbe969b593'::uuid, '18fb2bc5-1607-44c1-9bfd-98725f366748'::uuid, 0, TIME '10:58:47', 'Credito', 150.00::numeric(18,2),  4),
    ('0731d18f-286b-4fd2-89d5-0f55398d11a5'::uuid, 'b0d73452-0d6a-44a5-bab1-f6ea8dddb8be'::uuid, 0, TIME '14:41:03', 'Debito',  600.00::numeric(18,2),  6)
) AS t(outbox_id, lancamento_id, dias_atras, hora, tipo_lancamento, valor, atraso_publicacao_segundos)
ON CONFLICT (id) DO NOTHING;
