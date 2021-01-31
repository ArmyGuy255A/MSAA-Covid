apt -y update
apt -y dist-upgrade
apt -y install unclutter vim ntpdate at-spi2-core libnotify-bin \
    mate-notification-daemon mate-notification-daemon-common
apt -y remove vlc geany thonny qpdfview xarchiver gpicview galculator mousepad
apt -y autoremove