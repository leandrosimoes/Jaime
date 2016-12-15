;$(function(){
    var $menu = $('#menu'),
        $body = $('html, body'),
        $document = $(document);
    
    function ativarMenu() {
        if($document.scrollTop() > 0) {
            $menu.fadeIn();
        } 
        
        if($document.scrollTop() < 1) {
            $menu.fadeOut();
        }
    }
    
    function changeSectionIndex() {
        if($document.scrollTop() > 40) {
            $('section').css('z-index', '1001');
        } 
        
        if($document.scrollTop() <= 40){
            $('section').css('z-index', '999');
        }
        
        if($document.width() < 1500){
            $('section').css('z-index', '1001');
        }
    }   
    
    $(window).scroll(function () {
        ativarMenu();
        changeSectionIndex();
    });
    
    $(document).ready(function () {
        ativarMenu();
        changeSectionIndex();
    });
    
    $('#icone-entrar > i').on('click', function(){
        $body.stop().animate({ scrollTop: $('#services-area').offset().top }, { easing: 'swing', queue: false, duration: 1000 });
    });
    
    setInterval(function(){
        $('#icone-entrar').effect('bounce');
    }, 4000);
    
    _.forEach($menu.find('.slide-anchor'), function(item){
        $(item).on('click', function(event) {
            var url = $(item).data('url');            
            var pos = url === '#home' ? 0 : $(url).offset().top;           
            
            $body.stop().animate({ scrollTop: pos }, { easing: 'swing', queue: false, duration: 1000 });
            
            return false;
        });
    });
});