(() => {
  'use strict';
  const { api, escapeHtml, showMessage, formatDate } = window.Member5; const list=document.getElementById('interview-list'); const message=document.getElementById('message');
  async function load(){try{const data=await api('/api/Interviews/candidate?page=1&pageSize=100');const items=data.items||[];if(!items.length){list.className='member5-empty';list.textContent='No interviews yet.';return;}list.className='member5-grid';list.innerHTML=items.map(item=>`<article class="member5-card"><span class="member5-status">${escapeHtml(item.status)}</span><h3>${escapeHtml(item.companyName||item.companyId)}</h3><p>${escapeHtml(item.jobTitle||'Application')}</p><p class="member5-muted">${formatDate(item.scheduledAtUtc)} · ${escapeHtml(item.mode)} · ${item.durationMinutes} min</p>${item.meetingLink?`<p><a href="${escapeHtml(item.meetingLink)}" target="_blank" rel="noopener noreferrer">Open meeting link</a></p>`:`<p>${escapeHtml(item.location||item.contactPhone||'Details pending')}</p>`}</article>`).join('');}catch(error){list.className='member5-empty';list.textContent='Unable to load interviews.';showMessage(message,error.message);}}
  load();
})();
