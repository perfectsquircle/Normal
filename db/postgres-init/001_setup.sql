CREATE USER greglow;
CREATE USER azure_pg_admin;
CREATE USER azure_superuser;
CREATE USER toadstool WITH LOGIN;
ALTER USER toadstool WITH PASSWORD 'toadstool';
CREATE DATABASE wide_world_importers_pg;

GRANT ALL ON DATABASE wide_world_importers_pg TO toadstool;
GRANT ALL ON SCHEMA public, application, warehouse TO toadstool;
GRANT ALL ON ALL TABLES IN SCHEMA public, application, warehouse TO toadstool;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public, application, warehouse TO toadstool;
GRANT ALL ON ALL FUNCTIONS IN SCHEMA public, application, warehouse TO toadstool;
