#!/bin/bash
set -ex

dump_file=/tmp/wide_world_importers_pg.dump

pg_restore -U postgres -w -v -Fc -1 -d wide_world_importers_pg $dump_file