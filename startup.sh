#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Install dependencies to ensure everything is loaded properly
pip install --no-cache-dir --user --force-reinstall -r requirements.txt

# Optimize Gunicorn settings for Azure
exec gunicorn --bind 0.0.0.0:8000 --timeout 300 --workers 1 --worker-class=sync --log-level debug --access-logfile /home/logs/access.log --error-logfile /home/logs/error.log app:app

