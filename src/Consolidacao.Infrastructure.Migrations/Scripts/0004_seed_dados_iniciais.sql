-- Saldos diários coerentes com os lançamentos semeados em
-- Lancamentos.Infrastructure.Migrations/Scripts/0005_seed_dados_iniciais.sql:
--
--   Anteontem: 1200.00 + 350.50 (créditos) - 400.00 (débito)  = 1150.50
--   Ontem:     800.00 (crédito) - 250.75 - 120.00 (débitos)   =  429.25
--   Hoje:      2000.00 + 150.00 (créditos) - 600.00 (débito)  = 1550.00

INSERT INTO saldos_diarios (id, data, total_creditos, total_debitos, saldo)
SELECT
    id,
    (CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date,
    total_creditos,
    total_debitos,
    saldo
FROM (VALUES
    ('30dee218-c6c9-4ee7-a6f5-d5ec88100af2'::uuid, 2, 1550.50::numeric(18,2), 400.00::numeric(18,2), 1150.50::numeric(18,2)),
    ('e9f9da6f-1c0f-41b3-9e2c-5395909726fc'::uuid, 1, 800.00::numeric(18,2),  370.75::numeric(18,2),  429.25::numeric(18,2)),
    ('a20dedd5-3f99-42f2-b1da-b76fbdcf23ed'::uuid, 0, 2150.00::numeric(18,2), 600.00::numeric(18,2), 1550.00::numeric(18,2))
) AS t(id, dias_atras, total_creditos, total_debitos, saldo)
ON CONFLICT (data) DO NOTHING;

-- Marca os mesmos lançamentos do outro serviço como já processados. O
-- processado_em de cada um cai no mesmo dia do lançamento correspondente
-- (não em "agora"), poucos segundos depois da publicação no outbox — o tempo
-- que o consumer levaria para reagir à mensagem em uma execução real.
INSERT INTO lancamentos_processados (lancamento_id, processado_em)
SELECT
    lancamento_id,
    (CURRENT_DATE - (dias_atras * INTERVAL '1 day'))::date + hora + (atraso_total_segundos || ' seconds')::interval
FROM (VALUES
    ('81548384-4c74-444f-b1d9-09c470f01589'::uuid, 2, TIME '09:14:22', 6),
    ('9e7bbb40-4eac-4dae-83aa-d22785c52479'::uuid, 2, TIME '11:38:07', 7),
    ('bd2778f7-580f-4bbe-a5a7-fb1b224ecd6d'::uuid, 2, TIME '15:52:41', 6),

    ('806810d7-26fb-4e52-bcc7-b53041882d47'::uuid, 1, TIME '08:47:15', 8),
    ('adf7f494-4423-4cb6-a875-f5c349ec143c'::uuid, 1, TIME '13:05:38', 7),
    ('f21cee3f-80ed-4c46-8782-954fd069501a'::uuid, 1, TIME '17:22:59', 7),

    ('d050f4f0-5cbd-45cb-b28d-6eae171340e7'::uuid, 0, TIME '09:30:12', 7),
    ('18fb2bc5-1607-44c1-9bfd-98725f366748'::uuid, 0, TIME '10:58:47', 5),
    ('b0d73452-0d6a-44a5-bab1-f6ea8dddb8be'::uuid, 0, TIME '14:41:03', 7)
) AS t(lancamento_id, dias_atras, hora, atraso_total_segundos)
ON CONFLICT (lancamento_id) DO NOTHING;
