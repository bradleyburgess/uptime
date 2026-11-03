CREATE TABLE IF NOT EXISTS ping_result (
    type VARCHAR(100) PRIMARY KEY,
    last_date TIMESTAMP,
    success BOOLEAN
);

INSERT INTO ping_result (type) VALUES
    ('MostRecent'),
    ('MostRecentSuccess')
ON CONFLICT(type) DO NOTHING;
