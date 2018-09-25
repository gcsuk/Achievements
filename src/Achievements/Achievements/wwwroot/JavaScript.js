const { HubConnectionBuilder } = require('@aspnet/signalr');

const setupConnection = () => {
    connection = new HubConnectionBuilder()
        .withUrl(`/achievementshub?userId=1`)
        .build();

    connection.on('Unlocked', (message) => {
        console.log(`achievement unlocked: ${message}`);
    });
};

const start = async () => {
    await connection.start().catch(err => console.error(err.toString()));
    document.getElementById('start').disabled = true;
    document.getElementById('stop').disabled = false;
    document.getElementById('notify').disabled = false;
};

const stop = async () => {
    await connection.stop().catch(err => console.error(err.toString()));
    document.getElementById('start').disabled = false;
    document.getElementById('stop').disabled = true;
    document.getElementById('notify').disabled = true;
};

const notify = async () => {
    connection.invoke('MonitorAchievements', '1');
};

document.getElementById('start').addEventListener('click', start);
document.getElementById('stop').addEventListener('click', stop);
document.getElementById('notify').addEventListener('click', notify);

setupConnection();