#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Force reinstall of numpy and pandas to ensure compatibility
pip uninstall -y numpy pandas
pip install --no-cache-dir --user numpy pandas

# Log installed packages
pip list | tee /home/logs/packages.log

# Start Gunicorn
exec gunicorn --bind 0.0.0.0:8000 --timeout 6000 --workers 1 app:app
