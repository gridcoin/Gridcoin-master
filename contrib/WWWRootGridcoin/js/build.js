$( document ).ready(function() {

	//Mobile Navigation
	$('#mobileNav').click(function(){
		$('#mobileNavigation').slideToggle(1000);
	});
	
	// Support Accordian
	$("ul#faqList li").click(function(){
	
		// Check if the open slide was clicked
		if($(this).children('p').hasClass('open')) { 
		
			$(this).children('p').slideUp(1000);
			$('.open').removeClass("open").addClass("closed");
			$('.closed').parents().children('i').removeClass('fa-chevron-up').addClass('fa-chevron-down');
		
		} else {
		
			// Close any open slides
			$('.open').slideUp(1000);
			
			// Change the class of the open slides to closed
			$('.open').removeClass("open").addClass("closed");
			
			// Toggle the clicked slide
			$(this).children('p').slideToggle(1000, function () { 
			
			// Change the arrow
			if($(this).is(":visible")) {
			    $(this).parents().children('i').removeClass('fa-chevron-down').addClass('fa-chevron-up');
			}
			  
			});
	
			// Change the class of the opened slide to open
			$(this).children('p').removeClass("closed").addClass("open");
			
			// Change the arrow
			$('.closed').parents().children('i').removeClass('fa-chevron-up').addClass('fa-chevron-down');
		}
	});
});

$(window).load(function() {
        $('.mainSlider .slider').nivoSlider({effect: 'random'});
});

//Executes your code when the DOM is ready.  Acts the same as $(document).ready().
   $(function() {
     //Calls the selectBoxIt method on your HTML select box.
     $("select").selectBoxIt();
   });

// Google Maps
function initialise() {
         
    var myLatlng = new google.maps.LatLng(51.495412,-0.145876); // Add the coordinates
    var mapOptions = {
        zoom: 16, // The initial zoom level when your map loads (0-20)
        center: myLatlng, // Centre the Map to our coordinates variable
        mapTypeId: google.maps.MapTypeId.ROADMAP, // Set the type of Map
      }
    var pinIcon = new google.maps.MarkerImage(
        "http://chart.apis.google.com/chart?chst=d_map_pin_letter&chld=%E2%80%A2|2CB489",
        null, /* size is determined at runtime */
        null, /* origin is 0,0 */
        null, /* anchor is bottom center of the scaled image */
        new google.maps.Size(20, 30)
    );  
    var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions); // Render our map within the empty div
    var marker = new google.maps.Marker({ // Set the marker
    	icon: pinIcon,
        position: myLatlng, // Position marker to coordinates
        map: map, // assign the market to our map variable
    });
         
}
google.maps.event.addDomListener(window, 'load', initialise); // Execute our 'initialise' function once the page has loaded. 