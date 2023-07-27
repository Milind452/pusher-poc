// Enable pusher logging - don't include this in production
Pusher.logToConsole = true;

// Add appKey and cluster for your account
const pusher = new Pusher(<appKey>, {
    cluster: <cluster>,
});

const channelName = "channel-1";
const eventName = "event";

const channel = pusher.subscribe(channelName);

const notificationList = document.getElementById("notifications");
const notifications = [];

channel.bind(eventName, function (data) {
    notifications.unshift(data);
    renderNotifications();
});

function renderNotifications() {
    notifications.forEach((message) => {
        const notification = document.createElement("li");
        notification.className = "notification";

        const channelElement = document.createElement("div");
        channelElement.textContent = message.Channel;

        const eventElement = document.createElement("div");
        eventElement.textContent = message.Event;

        const messageElement = document.createElement("div");
        messageElement.textContent = message.Message;

        const timeElement = document.createElement("div");
        timeElement.textContent = new Date().toLocaleString();

        notification.appendChild(channelElement);
        notification.appendChild(eventElement);
        notification.appendChild(messageElement);
        notification.appendChild(timeElement);

        notificationList.appendChild(notification);
    });
}
