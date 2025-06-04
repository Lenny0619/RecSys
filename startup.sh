#!/bin/bash

cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Force installation of all dependencies including pandas
pip install --no-cache-dir --user --force-reinstall -r requirements.txt

# Log installed packages
pip list | tee /home/logs/packages.log

# Start Gunicorn
exec gunicorn --bind 0.0.0.0:8000 --timeout 120 --workers 2 app:app
