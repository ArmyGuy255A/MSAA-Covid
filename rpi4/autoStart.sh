#!/bin/bash

# URL to open automatically
URL=https://mssacovidkiosk.azurewebsites.net/

while ! ip route | grep -q -e "eth0" -e "wlan0"; do
    notify-send -t 900 "Waiting for network connection..." &> /dev/null
    sleep 1
done

notify-send -t 500 "Connected." &> /dev/null
notify-send -t 3000 "Starting browser..." &> /dev/null
chromium-browser --incognito --app=$URL \
    --start-fullscreen \
    --check-for-update-interval=31536000 \
    --overscroll-history-navigation=0 \
    --disable-pinch