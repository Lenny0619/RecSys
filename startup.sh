#!/bin/bash
cd /home/site/wwwroot/RecSys

# Initialize virtual environment
if [ ! -d "/home/site/wwwroot/venv" ]; then
    python -m venv /home/site/wwwroot/venv
fi

# Activate virtual environment
source /home/site/wwwroot/venv/bin/activate

# Upgrade pip
pip install --upgrade pip

# Install system dependencies required for pandas & Meson
apt-get update && apt-get install -y build-essential python3-dev libffi-dev libssl-dev

# Force reinstall numpy & pandas to prevent compatibility issues
pip uninstall -y numpy pandas
pip install numpy==1.26.4 pandas==2.2.2 --no-cache-dir --force-reinstall

# Install all dependencies from requirements.txt
pip install --no-cache-dir -r /home/site/wwwroot/RecSys/requirements.txt

# Verify installation of critical packages
python -c "import numpy, pandas; print(f'numpy:{numpy.__version__}, pandas:{pandas.__version__}')" > /home/logs/package_versions.log

# Start Gunicorn with optimized settings for Azure B3 plan
exec gunicorn --bind 0.0.0.0:8000 \
              --timeout 600 \
              --workers 2 \
              --threads 2 \
              --worker-class gthread \
              --access-logfile - \
              --error-logfile - \
              app:app
