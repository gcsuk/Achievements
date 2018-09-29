const { HubConnectionBuilder } = require('@aspnet/signalr');

const setupConnection = () => {
    connection = new HubConnectionBuilder()
        .withUrl(`/achievementshub?userId=1`)
        .build();

    connection.on('Unlocked', (achievement) => {
        console.log(`${achievement}`);
    });
};

const start = async () => {
    await connection.start();
    
    document.getElementById('start').disabled = true;
    document.getElementById('stop').disabled = false;
};

const stop = async () => {
    await connection.stop();

    document.getElementById('start').disabled = false;
    document.getElementById('stop').disabled = true;
};

document.getElementById('start').addEventListener('click', start);
document.getElementById('stop').addEventListener('click', stop);

setupConnection();