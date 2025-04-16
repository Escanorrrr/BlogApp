function handleImageUpload(input, previewId, hiddenInputId) {
    if (input.files && input.files[0]) {
        const file = input.files[0];
        const reader = new FileReader();
        
        reader.onload = function(e) {
            const base64String = e.target.result;
            
            // Base64 stringi hidden input'a kaydet
            document.getElementById(hiddenInputId).value = base64String;
            
            // Önizleme göster
            const previewImage = document.getElementById(previewId);
            if (previewImage) {
                previewImage.src = base64String;
                previewImage.style.display = 'block';
            }

            // API'ye gönder
            uploadImageToServer(base64String);
        };
        
        reader.readAsDataURL(file);
    }
}

async function uploadImageToServer(base64String) {
    try {
        const response = await fetch('/api/Foto/upload', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(base64String)
        });

        if (!response.ok) {
            throw new Error('Fotoğraf yükleme hatası');
        }

        const result = await response.json();
        return result.path;
    } catch (error) {
        console.error('Fotoğraf yükleme hatası:', error);
        return null;
    }
} 