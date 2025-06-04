#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate
exec gunicorn --bind 0.0.0.0:8000 --timeout 120 --workers 2 app:app