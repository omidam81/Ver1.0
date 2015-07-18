(function () {
    var filesUpload = document.getElementById("file"),
		dropArea = document.getElementById("dropbox"),
		fileType 	= ["png", "gif", "jpg", "jpeg"],
		maxsize		= 5;
	function uploadFile (file) {
		var ext = file.name.substr(file.name.lastIndexOf('.') + 1);
		var check = fileType.indexOf(ext);//alert(file.type);		
		if (check === -1) {
		    $('#incorrect-file-type').modal('show');
		    return false;
		}
		if (file.size > 1048576 * maxsize) {	//1048576 = 1MB
		    $('#file-size-too-big').modal('show');
		    return false;
		}
		
		/*
			If the file is an image and the web browser supports FileReader,
			present a preview in the file list
		*/
	    console.log(typeof window.FileReader, file.type);
		if (typeof window.FileReader !== "undefined" && (file.type === 'image/png' || file.type === 'image/jpg' || file.type === 'image/jpeg' || file.type === 'image/gif')) {
			var reader = new FileReader();
			console.log("reader");
			reader.onload = function (evt) {
			    console.log(evt);

			    var type = file.type.split('/')[1];
			    design.myart.create({ item: { url: evt.target.result, file_type: type } });
			};
			reader.readAsDataURL(file);
		}
		else
		{
		    // Uploading - for Firefox, Google Chrome and Safari
		    var xhr = new XMLHttpRequest();

		    // Update progress bar
		    xhr.upload.addEventListener("progress", function (evt) {
		        if (evt.lengthComputable) {
		            var completed = (evt.loaded / evt.total) * 100;
		        }
		        else {
		            // No data to calculate on
		        }
		    }, false);

		    var url = '/UpoadArtFile';

		    xhr.open("post", url, true);

		    xhr.onload = function () {
		        var media = eval('(' + this.responseText + ')');
		        console.log(media);
		        //span.setAttribute('onclick', 'dagFiles.file.select(this)');			
		        //img.setAttribute('src', media.url);			
		    };

		    var formData = new FormData();
		    formData.append('file', file);
		    formData.append('__RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
		    xhr.send(formData);
		}
		
    }
	
	function traverseFiles (files) {
		if (files) {
			for (var i=0, l=files.length; i<l; i++) {
				uploadFile(files[i]);
			}
		}
	}

	filesUpload.addEventListener("change", function () {
	    traverseFiles(this.files);
	}, false);

	dropArea.addEventListener("dragleave", function (evt) {
		var target = evt.target;
		
		if (target && target === dropArea) {
			this.className = "";
		}
		evt.preventDefault();
		evt.stopPropagation();
	}, false);
	
	dropArea.addEventListener("dragenter", function (evt) {
		this.className = "over";
		evt.preventDefault();
		evt.stopPropagation();
	}, false);
	
	dropArea.addEventListener("dragover", function (evt) {
		evt.preventDefault();
		evt.stopPropagation();
	}, false);
	
	dropArea.addEventListener("drop", function (evt) {
		traverseFiles(evt.dataTransfer.files);
		this.className = "";
		evt.preventDefault();
		evt.stopPropagation();
	}, false);										
})();