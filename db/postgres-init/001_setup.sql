CREATE USER greglow;
CREATE USER azure_pg_admin;
CREATE USER azure_superuser;
CREATE USER toadstool WITH LOGIN;
ALTER USER toadstool WITH PASSWORD 'toadstool';
CREATE DATABASE wide_world_importers_pg;

GRANT ALL PRIVILEGES ON DATABASE wide_world_importers_pg TO toadstool;
GRANT ALL PRIVILEGES ON SCHEMA application TO toadstool;
GRANT ALL PRIVILEGES ON SCHEMA warehouse TO toadstool;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA application TO toadstool;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA application TO toadstool;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA warehouse TO toadstool;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA warehouse TO toadstool;
