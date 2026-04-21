function initMessaging(selectedUserId) {
    const chatContainer = document.getElementById("chatContainer");

    const scrollToBottom = () => {
        if (chatContainer) chatContainer.scrollTop = chatContainer.scrollHeight;
    };

    scrollToBottom();

    document.getElementById('userSearch')?.addEventListener('keyup', function() {
        let filter = this.value.toLowerCase();
        document.querySelectorAll('.list-group-item').forEach(item => {
            let name = item.querySelector('h6').textContent.toLowerCase();
            item.style.display = name.includes(filter) ? "" : "none";
        });
    });

    if (selectedUserId) {
        setInterval(() => {
            fetch(window.location.href)
                .then(response => response.text())
                .then(html => {
                    const parser = new DOMParser();
                    const doc = parser.parseFromString(html, 'text/html');
                    const newNode = doc.getElementById('chatContainer');
                    const currentNode = document.getElementById('chatContainer');

                    if (newNode && currentNode && newNode.innerHTML.trim() !== currentNode.innerHTML.trim()) {
                        currentNode.innerHTML = newNode.innerHTML;
                        scrollToBottom();
                    }
                })
                .catch(err => console.warn("Polling error:", err));
        }, 2000); 
    }
}