const apiBase = 'https://localhost:7001/api/v1/';

function updateDateTime() {
    const now = new Date();
    const bn = ['রবিবার', 'সোমবার', 'মঙ্গলবার', 'বুধবার', 'বৃহস্পতিবার', 'শুক্রবার', 'শনিবার'];
    const months = ['জানুয়ারি', 'ফেব্রুয়ারি', 'মার্চ', 'এপ্রিল', 'মে', 'জুন',
        'জুলাই', 'আগস্ট', 'সেপ্টেম্বর', 'অক্টোবর', 'নভেম্বর', 'ডিসেম্বর'];

    const dateStr = `${bn[now.getDay()]}, ${now.getDate()} ${months[now.getMonth()]} ${now.getFullYear()}`;
    const h = now.getHours(), m = now.getMinutes();
    const ampm = h >= 12 ? 'PM' : 'AM';
    const h12 = h % 12 || 12;
    const timeStr = `${String(h12).padStart(2, '0')}:${String(m).padStart(2, '0')} ${ampm}`;

    const dateEl = document.getElementById('live-date');
    const timeEl = document.getElementById('live-time');
    if (dateEl) dateEl.textContent = dateStr;
    if (timeEl) timeEl.textContent = timeStr;
}

updateDateTime();
setInterval(updateDateTime, 60000);

window.addEventListener('scroll', function () {
    const btn = document.getElementById('back-to-top');
    if (!btn) return;
    btn.style.display = window.scrollY > 400 ? 'flex' : 'none';
});

const searchInput = document.getElementById('live-search-input');
const searchDropdown = document.getElementById('search-dropdown');
const searchBtn = document.getElementById('search-btn');
let searchTimer = null;

if (searchInput) {
    searchInput.addEventListener('input', function () {
        clearTimeout(searchTimer);
        const q = this.value.trim();
        if (q.length < 2) {
            searchDropdown.style.display = 'none';
            return;
        }
        searchTimer = setTimeout(() => liveSearch(q), 400);
    });

    searchInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            const q = this.value.trim();
            if (q) window.location.href = `/news/search?q=${encodeURIComponent(q)}`;
        }
        if (e.key === 'Escape') {
            searchDropdown.style.display = 'none';
        }
    });

    document.addEventListener('click', function (e) {
        if (!e.target.closest('#live-search-input') &&
            !e.target.closest('#search-dropdown')) {
            searchDropdown.style.display = 'none';
        }
    });
}

if (searchBtn) {
    searchBtn.addEventListener('click', function () {
        const q = searchInput?.value.trim();
        if (q) window.location.href = `/news/search?q=${encodeURIComponent(q)}`;
    });
}

async function liveSearch(q) {
    searchDropdown.style.display = 'block';
    searchDropdown.innerHTML = '<div class="search-loading"><i class="fas fa-spinner fa-spin"></i> খোঁজা হচ্ছে...</div>';

    try {
        const res = await fetch(
            `${apiBase}news/search?q=${encodeURIComponent(q)}&page=1&pageSize=5`);
        const data = await res.json();

        if (!data.success || !data.data?.items?.length) {
            searchDropdown.innerHTML =
                '<div class="search-empty">কোনো সংবাদ পাওয়া যায়নি</div>';
            return;
        }

        searchDropdown.innerHTML = data.data.items.map(item => `
            <a href="/news/${item.slug}" class="search-item">
                ${item.thumbnailUrl
                ? `<img src="${item.thumbnailUrl}" alt="${item.titleBn}"/>`
                : `<div style="width:56px;height:38px;background:#f5f5f5;border-radius:4px;flex-shrink:0;"></div>`}
                <div class="search-item-body">
                    <p class="search-item-title">${item.titleBn}</p>
                    <span class="search-item-cat">${item.categoryNameBn}</span>
                </div>
            </a>
        `).join('') +
            `<a href="/news/search?q=${encodeURIComponent(q)}" class="search-more">
            সব ফলাফল দেখুন →
        </a>`;
    } catch {
        searchDropdown.innerHTML =
            '<div class="search-empty">সার্চ করা যায়নি</div>';
    }
}

const apiBase = 'https://localhost:7001/api/v1/';

function getArticleId() {
    const el = document.querySelector('[id^="view-count-"]');
    return el ? parseInt(el.id.replace('view-count-', '')) : null;
}

async function loadComments() {
    const articleId = getArticleId();
    const list = document.getElementById('comments-list');
    if (!articleId || !list) return;

    try {
        const res = await fetch(`${apiBase}comment/article/${articleId}`);
        const data = await res.json();

        if (!data.success || !data.data?.length) {
            list.innerHTML = `
                <div style="text-align:center;padding:24px 0;color:#aaa;">
                    <i class="fas fa-comments" style="font-size:28px;opacity:0.3;display:block;margin-bottom:8px;"></i>
                    <p style="font-size:14px;margin:0;">এখনো কোনো মন্তব্য নেই। প্রথম মন্তব্য করুন!</p>
                </div>`;
            return;
        }

        list.innerHTML = `
            <h4 style="font-size:15px;color:#555;margin:0 0 16px;border-top:0.5px solid #f0f0f0;padding-top:16px;">
                ${data.data.length} টি মন্তব্য
            </h4>
            ${data.data.map(c => renderComment(c)).join('')}`;
    } catch {
        const list = document.getElementById('comments-list');
        if (list) list.innerHTML = '';
    }
}

function renderComment(c) {
    const initial = c.userNameBn?.[0] || c.userName?.[0] || 'ব';
    const name = c.userNameBn || c.userName || 'পাঠক';
    const date = new Date(c.createdAt).toLocaleDateString('bn-BD', {
        year: 'numeric', month: 'long', day: 'numeric'
    });

    const repliesHtml = c.replies?.length ? `
        <div style="margin:12px 0 0 48px;">
            ${c.replies.map(r => `
                <div style="padding:10px 14px;background:#fef9f9;border-radius:6px;border-left:3px solid #c0392b;margin-bottom:8px;">
                    <p style="font-size:13px;font-weight:500;color:#444;margin:0 0 4px;">
                        ${r.userNameBn || r.userName}
                    </p>
                    <p style="font-size:14px;color:#555;margin:0;line-height:1.6;">
                        ${r.content}
                    </p>
                </div>
            `).join('')}
        </div>` : '';

    return `
        <div style="padding:16px 0;border-bottom:0.5px solid #f5f5f5;">
            <div style="display:flex;gap:12px;">
                <div style="width:40px;height:40px;border-radius:50%;background:#c0392b;color:white;display:flex;align-items:center;justify-content:center;font-size:16px;font-weight:600;flex-shrink:0;">
                    ${initial}
                </div>
                <div style="flex:1;">
                    <div style="display:flex;align-items:center;gap:10px;margin-bottom:6px;">
                        <span style="font-size:14px;font-weight:500;color:#222;">
                            ${name}
                        </span>
                        <span style="font-size:12px;color:#aaa;">${date}</span>
                    </div>
                    <p style="font-size:15px;color:#333;line-height:1.7;margin:0;">
                        ${c.content}
                    </p>
                    ${repliesHtml}
                </div>
            </div>
        </div>`;
}


//
async function subscribeNewsletter() {
    const email = document.getElementById(
        'newsletter-email')?.value?.trim();
    const btn = document.getElementById('newsletter-btn');
    const msg = document.getElementById('newsletter-msg');

    if (!email) {
        if (msg) msg.textContent = 'ইমেইল দিন';
        return;
    }

    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
        if (msg) msg.textContent = 'সঠিক ইমেইল দিন';
        return;
    }

    btn.disabled = true;
    btn.textContent = 'পাঠানো হচ্ছে...';

    try {
        const res = await fetch(
            `${apiBase}newsletter/subscribe`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email })
        });

        const data = await res.json();
        if (msg) {
            msg.textContent = data.message ||
                'সাবস্ক্রিপশন সফল হয়েছে!';
        }

        const input = document.getElementById(
            'newsletter-email');
        if (input) input.value = '';
    } catch {
        if (msg) msg.textContent = 'সংযোগ সমস্যা হয়েছে';
    } finally {
        btn.disabled = false;
        btn.textContent = 'সাবস্ক্রাইব';
    }
}


async function submitComment() {
    const articleId = getArticleId();
    const content = document.getElementById('comment-content')?.value?.trim();
    const name = document.getElementById('comment-name')?.value?.trim();
    const btn = document.getElementById('comment-submit-btn');
    const msg = document.getElementById('comment-message');

    if (!name) {
        showCommentMsg('আপনার নাম দিন', 'error');
        return;
    }

    if (!content || content.length < 5) {
        showCommentMsg('কমপক্ষে ৫ অক্ষরের মন্তব্য লিখুন', 'error');
        return;
    }

    if (!articleId) return;

    btn.disabled = true;
    btn.textContent = 'পাঠানো হচ্ছে...';

    try {
        const res = await fetch(`${apiBase}comment/guest`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                articleId,
                content,
                guestName: name,
                guestEmail: document.getElementById('comment-email')?.value?.trim() || ''
            })
        });

        const data = await res.json();

        if (data.success) {
            document.getElementById('comment-content').value = '';
            document.getElementById('comment-name').value = '';
            document.getElementById('comment-email').value = '';
            showCommentMsg(
                'মন্তব্য পাঠানো হয়েছে। অনুমোদনের পর দেখা যাবে।',
                'success');
        } else {
            showCommentMsg(data.message || 'মন্তব্য পাঠানো যায়নি', 'error');
        }
    } catch {
        showCommentMsg('সংযোগ সমস্যা। আবার চেষ্টা করুন।', 'error');
    } finally {
        btn.disabled = false;
        btn.textContent = 'মন্তব্য পাঠান';
    }
}


async function loadHomeVideos() {
    const section = document.getElementById('video-section');
    const grid = document.getElementById('home-video-grid');
    if (!section || !grid) return;

    try {
        const res = await fetch(`${apiBase}video/featured?count=4`);
        const data = await res.json();
        if (!data.success || !data.data?.length) return;

        section.style.display = 'block';

        grid.innerHTML = data.data.map(v => `
            <a href="/video/watch/${v.id}"
               style="display:block;text-decoration:none;background:white;border-radius:8px;overflow:hidden;border:0.5px solid #e0e0e0;transition:transform .2s;"
               onmouseover="this.style.transform='translateY(-3px)'"
               onmouseout="this.style.transform='translateY(0)'">
                <div style="position:relative;background:#111;">
                    <img src="${v.thumbnailUrl || ''}"
                         loading="lazy"
                         style="width:100%;height:120px;object-fit:cover;display:block;opacity:0.9;"
                         onerror="this.style.display='none'"/>
                    <div style="position:absolute;top:50%;left:50%;transform:translate(-50%,-50%);width:36px;height:36px;background:rgba(192,57,43,0.9);border-radius:50%;display:flex;align-items:center;justify-content:center;">
                        <i class="fas fa-play" style="color:white;font-size:12px;margin-left:2px;"></i>
                    </div>
                    <span style="position:absolute;bottom:6px;right:6px;background:rgba(0,0,0,0.7);color:white;font-size:10px;padding:2px 6px;border-radius:3px;">
                        <i class="fas fa-eye" style="font-size:8px;"></i> ${v.viewCount}
                    </span>
                </div>
                <div style="padding:10px;">
                    <span style="background:#fde8e8;color:#c0392b;font-size:10px;padding:1px 6px;border-radius:10px;">
                        ${v.categoryNameBn || 'ভিডিও'}
                    </span>
                    <p style="font-size:13px;font-weight:500;color:#1a1a1a;margin:5px 0 0;line-height:1.4;display:-webkit-box;-webkit-line-clamp:2;-webkit-box-orient:vertical;overflow:hidden;">
                        ${v.titleBn}
                    </p>
                </div>
            </a>
        `).join('');
    } catch (e) {
        console.log('Video load error:', e);
    }
}

function showCommentMsg(text, type) {
    const msg = document.getElementById('comment-message');
    if (!msg) return;
    msg.textContent = text;
    msg.style.display = 'block';
    msg.style.background = type === 'success' ? '#d4edda' : '#fde8e8';
    msg.style.color = type === 'success' ? '#155724' : '#c0392b';
    setTimeout(() => { msg.style.display = 'none'; }, 5000);
}

document.addEventListener('DOMContentLoaded', loadComments);


document.addEventListener('DOMContentLoaded', function () {
    const images = document.querySelectorAll('img[src]');

    if ('IntersectionObserver' in window) {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;

                    img.style.opacity = '0';
                    img.style.transition = 'opacity .3s';

                    if (img.complete) {
                        // already loaded
                        img.style.opacity = '1';
                    } else {
                        img.onload = () => img.style.opacity = '1';
                    }

                    observer.unobserve(img);
                }
            });
        }, { rootMargin: '100px' });

        images.forEach(img => observer.observe(img));
    }
});