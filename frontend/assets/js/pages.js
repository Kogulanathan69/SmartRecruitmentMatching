const API_BASE = "https://localhost:7000/api";

function getToken() {
    return localStorage.getItem("token");
}

async function apiRequest(url, options = {}) {

    const token = getToken();

    const response = await fetch(`${API_BASE}${url}`, {
        ...options,
        headers: {
            ...(options.headers || {}),
            "Authorization": `Bearer ${token}`
        }
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }

    if (response.status === 204)
        return null;

    return response.json();
}
async function loadCompanyProfile() {
    return await apiRequest("/Companies/me");
}
async function saveCompanyProfile(data) {

    return await apiRequest("/Companies/me", {
        method: "PUT",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });
}
async function submitCompanyVerification() {

    return await apiRequest(
        "/Companies/verification/submit",
        {
            method: "POST"
        }
    );
}
async function createJob(data) {

    return await apiRequest("/Jobs", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });
}
async function publishJob(jobId) {

    return await apiRequest(
        `/Jobs/${jobId}/publish`,
        {
            method: "POST"
        }
    );
}
async function closeJob(jobId) {

    return await apiRequest(
        `/Jobs/${jobId}/close`,
        {
            method: "POST"
        }
    );
}
async function deleteJob(jobId) {

    return await apiRequest(
        `/Jobs/${jobId}`,
        {
            method: "DELETE"
        }
    );
}
// ===============================
// COMPANY DASHBOARD
// ===============================

async function loadCompanyDashboard() {

    return await apiRequest(
        "/Dashboard/company"
    );
}
// ===============================
// COMPANY DOCUMENTS
// ===============================

async function uploadCompanyDocument(
    file,
    documentType
) {

    const formData = new FormData();

    formData.append("file", file);
    formData.append(
        "documentType",
        documentType
    );

    return await apiRequest(
        "/Companies/documents",
        {
            method: "POST",
            body: formData
        }
    );
}