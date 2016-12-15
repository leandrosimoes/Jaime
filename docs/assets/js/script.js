;$(function(){
    var $window = $(window),
        $document = $(document),
	    scrollTime = 1.2,
	    scrollDistance = 300,
        $services = $('.service'),
        $links = $('.link'),
        $tutorials = $('.tutorial-item'),
        $socialIcons = $('.social-icon-anchor'),
        $serviceArea = $('#services-area'),
        $downloadArea = $('#download-area'),
        $tutorialArea = $('#tutorials-area'),
        $doacoesArea = $('#donation-area'),
        $authorArea = $('#author-area'),
        cont = 0,
        mostrouServicos = false,
        mostrouTutoriais = false,
        mostrouIconesSociais = false,
        mostrouIconesDoacoes = false,
        mostrouLinks = false;

	$window.on("mousewheel DOMMouseScroll", function(event){
		event.preventDefault();	

		var delta = event.originalEvent.wheelDelta/120 || -event.originalEvent.detail/3,
		    scrollTop = $window.scrollTop(),
		    finalScroll = scrollTop - parseInt(delta*scrollDistance);

		TweenMax.to($window, scrollTime, {
			scrollTo : { y: finalScroll, autoKill:true },
				ease: Power1.easeOut,
				overwrite: 5							
        });
	});
    
    $(window).scroll(function () {
        mostrarOcultarServicos();
        mostrarOcultarLinks();
        mostrarOcultarTutoriais();
        mostrarOcultarSocial();
        mostrarOcultarDoacoes();
    });
    
    $(document).ready(function () {
        mostrarOcultarServicos();
        mostrarOcultarLinks();
        mostrarOcultarTutoriais();
        mostrarOcultarSocial();
        mostrarOcultarDoacoes();
    });
    
    $('.link-container').on('click', function(){
        var dataUrl = $(this).data('url');
        
        if(!!dataUrl) {
            if(dataUrl === 'download'){
                dataUrl = 'https://github.com/leandrosimoes/Jaime/releases/download/2.0.0/jaime.zip';
               if($('#idown').length ){
                  $('#idown').attr('src', dataUrl);
               } else {
                  $('<iframe>', { id:'idown', src: dataUrl }).hide().appendTo('body');
               }
            } else {
                window.open(dataUrl, '_blank');                
            }
            
        }
    });
    
    $('.item-title').on('click', function(){
        var itemTitle = $(this).data('item'),
            item = $(itemTitle);
        
        if(item.is(':visible'))
            item.stop().slideUp();
        else
            item.stop().slideDown();        
    });
    
    function mostrarOcultarLinks() {
        if($document.scrollTop() > $downloadArea.offset().top-10 && !mostrouLinks) {
            mostrouLinks = true;
            _.forEach($links, function(item){
                $(item).addClass('showed');
            });
        }
        
        if($document.scrollTop() < $downloadArea.offset().top-10 && mostrouLinks) {
            mostrouLinks = false;
            _.forEach($links, function(item){
                $(item).removeClass('showed');  
            });
        }
    }
    
    function mostrarOcultarServicos() {
        if($document.scrollTop() > $serviceArea.offset().top-10 && !mostrouServicos) {
            mostrouServicos = true;
            var cont = 1;
            _.forEach($services, function(item){
                $(item).addClass('showed'+cont);
                cont=cont+1;
            });
        }
        
        if($document.scrollTop() < $serviceArea.offset().top-10 && mostrouServicos) {
            mostrouServicos = false;
            _.forEach($services, function(item){
                item.className = 'service';
            });
        }
    }
    
    function mostrarOcultarTutoriais() {
        if($document.scrollTop() > $tutorialArea.offset().top-50 && !mostrouTutoriais) {
            mostrouTutoriais = true;
            var cont = 1;
            _.forEach($tutorials, function(item){
                $(item).addClass('showed'+cont);
                cont=cont+1;
            });
        }
        
        if($document.scrollTop() < $tutorialArea.offset().top-50 && mostrouTutoriais) {
            mostrouTutoriais = false;
            _.forEach($tutorials, function(item){
                item.className = 'tutorial-item';
            });
        }
    }
    
    function mostrarOcultarSocial() {
        if($document.scrollTop() > $authorArea.offset().top-10 && !mostrouIconesSociais) {
            mostrouIconesSociais = true;
            var cont = 1;
            _.forEach($socialIcons, function(item){
                $(item).addClass('showed'+cont);
                cont=cont+1;
            });
        }
        
        if($document.scrollTop() < $authorArea.offset().top-10 && mostrouIconesSociais) {
            mostrouIconesSociais = false;
            _.forEach($socialIcons, function(item){
                item.className = 'social-icon-anchor';
            });
        }
    }
    
    function mostrarOcultarDoacoes() {
        if($document.scrollTop() > $authorArea.offset().top+10 && !mostrouIconesDoacoes) {
            mostrouIconesDoacoes = true;
            $doacoesArea.addClass('showed');
        }
        
        if($document.scrollTop() < $authorArea.offset().top+10 && mostrouIconesDoacoes) {
            mostrouIconesDoacoes = false;
            $doacoesArea.removeClass('showed');
        }
    }
    
    setInterval(function(){
        $('#pagseguro-button').effect('bounce');
    }, 4000);
    
    _.forEach($services, function(item){
        $(item).click(function(){
            $(this).effect('shake');
        });
    })
});