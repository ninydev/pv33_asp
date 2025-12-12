const messageInput = document.getElementById('messageInput');
const sendButton = document.getElementById('sendButton');
const messageList = document.getElementById('messageList');

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