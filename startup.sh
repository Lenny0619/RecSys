#!/bin/bash
cd /home/site/wwwroot/RecSys
source /home/site/wwwroot/venv/bin/activate

# Define dependencies to check before installing
DEPENDENCIES=(
    "Flask==2.3.2"
    "Werkzeug==2.3.7"
    "Jinja2==3.1.3"
    "itsdangerous==2.1.2"
    "Flask-Babel==4.0.0"
    "Flask-Cors==5.0.0"
    "Flask-SQLAlchemy==3.1.1"
    "SQLAlchemy==2.0.36"
    "APScheduler==3.10.1"
    "pyodbc==5.2.0"
    "pandas==2.0.3"
    "numpy==1.24.3"
    "scikit-learn==1.4.2"
    "joblib==1.4.0"
    "scikit-surprise==1.1.4"
    "gunicorn==23.0.0"
    "waitress==3.0.2"
)

# Check and install missing dependencies
for package in "${DEPENDENCIES[@]}"; do
    if ! pip list | grep -q "$(echo $package | cut -d= -f1)"; then
        echo "Installing missing package: $package"
        pip install --no-cache-dir --user "$package"
    else
        echo "$package is already installed, skipping."
    fi
done

# Log installed packages for debugging
pip list | tee /home/logs/packages.log

# Start Gunicorn with optimized settings
exec gunicorn --bind 0.0.0.0:8000 --timeout 600 --workers 1 app:app
