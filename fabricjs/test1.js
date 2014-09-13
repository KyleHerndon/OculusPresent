var fabric = require('fabric').fabric,
		http = require('http'),
		url = require('url'),
		PORT = 8124;

var server = http.createServer(function (request, response) {
	var params = url.parse(request.url, true);
	var canvas = fabric.createCanvasForNode(200, 200);

	response.writeHead(200, { 'Content-Type': 'image/png' });

	canvas.loadFromJSON(params.query.data, function() {
		canvas.renderAll();

		var stream = canvas.createPNGStream();
		stream.on('data', function(chunk) {
			response.write(chunk);
		});
		stream.on('end', function() {
			response.end();
		});
	});
});

server.listen(PORT);
