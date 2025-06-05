#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Install system dependencies required for Meson & pandas
apt-get install -y build-essential python3-dev libffi-dev libssl-dev

# Force reinstall numpy and pandas to ensure compatibility
pip uninstall -y numpy pandas
pip install --no-cache-dir --user numpy
pip install --no-cache-dir --user pandas==2.0.3 --no-build-isolation

# Log installed packages
pip list | tee /home/logs/packages.log

# Start Gunicorn
exec gunicorn --bind 0.0.0.0:8000 --timeout 6000 --workers 1 app:app
