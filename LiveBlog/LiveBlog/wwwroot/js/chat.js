const messageInput = document.getElementById('messageInput');
const sendButton = document.getElementById('sendButton');
const messageList = document.getElementById('messagesList');
const userName= messageList.dataset.userName;

function sendMessage(message) {
    fetch('/api/chat/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message })
    })
    .then((res) => { })
    .catch((err) => console.error(err));
    
}


sendButton.onclick = () => {
    const message = messageInput.value;
    messageInput.value = '';
    sendMessage(message);
}

window.eventSource.addEventListener('ChatMessageNotification', (e) => {
    const data = JSON.parse(e.data);
    console.log(data);
    const div = document.createElement('div');
    const t = new Date(Number(e.lastEventId)).toLocaleString();
    div.innerHTML = 
        "" +
        "<strong>" + data.fromUserName + "</strong>: " + data.message + 
        "<span class='text-muted small float-end'>" + t + "</span>";
    
    if (data.fromUserName === userName ) div.className= 'alert alert-info mb-2 text-end';
    else div.className ='alert alert-secondary mb-2';
    
    messageList.appendChild(div);
})