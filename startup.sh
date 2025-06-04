#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Log installed packages for debugging
pip list | tee /home/logs/packages.log

exec gunicorn --bind 0.0.0.0:8000 --timeout 120 --workers 2 app:app
