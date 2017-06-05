site.blink = function blink(selector, speed, iterations, counter) {
	counter = counter | 0; //this line is reason why external call lacks 4th param
	$(selector).animate({ opacity: 0 }, 50, "linear", function () {
		$(this).delay(speed);
		$(this).animate({ opacity: 1 }, 50, function () {
			counter++;
			if (iterations === -1) {
				blink(this, speed, iterations, counter);
			} else if (counter >= iterations) {
				return false;
			} else {
				blink(this, speed, iterations, counter);
			}
			return false;
		});
		$(this).delay(speed);
	});
};