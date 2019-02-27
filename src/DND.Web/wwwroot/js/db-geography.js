(function () {
    // Method signature matching $.fn.each()'s, for easy use in the .each loop later.
    var initialize = function (i, el) {
        // el is the input element that we need to initialize a map for, jQuery-ize it,
        //  and cache that since we'll be using it a few times.
        var $input = $(el);

        // Create the map div and insert it into the page.
        var $map = $('<div>', {

            css: {
                width: '100%',
                height: '400px'
            }
        }).insertAfter($input);

        var placeIdInput = null;

        if (document.getElementById('PlaceId') !== null)
        {
            placeIdInput = document.getElementById('PlaceId');
        }

        // Attempt to parse the lat/long coordinates out of this input element.
        var latLong = parseLatLong(this.value);

        // If there was a problem attaining a lat/long from the input element's value,
        //  set it to a sensible default that isn't in the middle of the ocean.
        if (!latLong || !latLong.latitude || !latLong.longitude) {
            latLong = {
                latitude: 46.8729599408635,
                longitude: 16.5243165940046
            };
        }

        // Create a "Google(r)(tm)" LatLong object representing our DBGeometry's lat/long.
        var position = new google.maps.LatLng(latLong.latitude, latLong.longitude);

        // Initialize the map widget.
        var map = new google.maps.Map($map[0], {
            zoom: 7,
            center: position,
            mapTypeId: 'roadmap'
        });

        // Place a marker on it, representing the DBGeometry object's position.
        var marker = new google.maps.Marker({
            position: position,
            map: map
        });

        var resized = false;
        $(document).ready(function () {
            $(window).resize(function () {
                google.maps.event.trigger(map, 'resize');
                map.setCenter(marker.getPosition());  
            });
            google.maps.event.trigger(map, 'resize');
            map.setCenter(marker.getPosition());  
        }); 

        var updateMarker = function (updateEvent) {
            marker.setPosition(updateEvent.latLng);
           
            // This new location might be outside the current viewport, especially
            //  if it was manually entered. Pan to center on the new marker location.
            map.panTo(updateEvent.latLng);

            // Black magic, courtesy of Hanselman's original version.
            $input.val(marker.getPosition().toUrlValue(13));
        };

        // If the input came from an EditorFor, initialize editing-related events.
        if ($input.hasClass('editor-for-dbgeography')) {
            google.maps.event.addListener(map, 'click', function (updateEvent) {
                updateMarker(updateEvent);
                if (placeIdInput !== null) {
                    placeIdInput.value = '';
                }
            });
            google.maps.event.addListener(map, 'click', function () {
                infowindow.close();
            });

            var $search = $('<input>', {
                id: 'map-search',
                class: 'controls',
                type: 'text',
                placeholder: 'Enter a location'
            }).insertAfter($input);
            var search = document.getElementById('map-search');

            var $infowindowContent = $('<div id="infowindow-content"><span id="place-name" class="title"></span><br>Place ID<span id="place-id"></span><br><span id="place-address"></span></div>').insertAfter($input);
            var infowindowContent = document.getElementById('infowindow-content');

            var autocomplete = new google.maps.places.Autocomplete(search);
            autocomplete.bindTo('bounds', map);

            map.controls[google.maps.ControlPosition.TOP_LEFT].push(search);

            var infowindow = new google.maps.InfoWindow();
            infowindow.setContent(infowindowContent);

            marker.addListener('click', function () {            
                infowindow.open(map, marker);
            });

            autocomplete.addListener('place_changed', function () {
                infowindow.close();
                var place = autocomplete.getPlace();
                if (!place.geometry) {
                    return;
                }

                if (place.geometry.viewport) {
                    map.fitBounds(place.geometry.viewport);
                } else {
                    map.setCenter(place.geometry.location);
                    map.setZoom(17);
                }


                var latLong = place.geometry.location;
                var placeId = place.place_id;

                if (placeIdInput !== null)
                {
                    placeIdInput.value = placeId;
                }

                // Set the position of the marker using the place ID and location.
                //marker.setPlace({
                //    //placeId: place.place_id,
                //    location: place.geometry.location
                //});

                updateMarker({ latLng: place.geometry.location });

                //inputOnChange = false;
                //$input.val(place.geometry.location.toUrlValue(13));
                //inputOnChange = true;

                marker.setVisible(true);

                infowindowContent.children['place-name'].textContent = place.name;
                infowindowContent.children['place-id'].textContent = place.place_id;
                infowindowContent.children['place-address'].textContent = place.formatted_address;
                infowindow.open(map, marker);
            });

            // Attempt to react to user edits in the input field.
            //$input.on('change', function () {
             
            //        var latLong = parseLatLong(this.value);

            //        latLong = new google.maps.LatLng(latLong.latitude, latLong.longitude);

            //        updateMarker({ latLng: latLong });
                         
            //});
        }
    };

    var parseLatLong = function (value) {
        if (!value) { return undefined; }

        var latLong = value.match(/-?\d+\.\d+/g);

        return {
            latitude: latLong[0],
            longitude: latLong[1]
        };
    };

    // Find all DBGeography inputs and initialize maps for them.
    $('.editor-for-dbgeography, .display-for-dbgeography').each(initialize);
})();