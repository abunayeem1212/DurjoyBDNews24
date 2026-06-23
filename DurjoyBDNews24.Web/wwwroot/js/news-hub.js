const hubUrl = document.querySelector('meta[name="hub-url"]')?.content
    || 'https://localhost:7001/hubs/news';

const connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .build();

connection.on('ReceiveBreakingTicker', (title, slug) => {
    const ticker = document.getElementById('breaking-ticker');
    if (!ticker) return;
    const item = document.createElement('span');
    item.className = 'ticker-item new-item';
    item.innerHTML = `<a href="/news/${slug}">${title}</a>`;
    ticker.prepend(item);
    setTimeout(() => item.classList.remove('new-item'), 3000);
});

connection.on('ReceiveNewArticle', (article) => {
    showToast(`নতুন সংবাদ: ${article.titleBn}`, article.slug);
});

connection.on('ReceiveViewCount', (articleId, count) => {
    const el = document.getElementById(`view-count-${articleId}`);
    if (el) el.textContent = `${count} বার পঠিত`;
});

function showToast(message, slug) {
    const toast = document.createElement('div');
    toast.className = 'news-toast';
    toast.innerHTML = `<a href="/news/${slug}">${message}</a>`;
    document.body.appendChild(toast);
    setTimeout(() => toast.classList.add('show'), 100);
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 500);
    }, 5000);
}

async function startConnection() {
    try {
        await connection.start();
        console.log('SignalR connected');
    } catch (err) {
        setTimeout(startConnection, 5000);
    }
}

startConnection();