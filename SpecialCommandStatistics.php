<?php
class SpecialCommandStatistics extends SpecialPage {
	function __construct() {
		parent::__construct( 'CommandStatistics' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		
		
		$wgOut->addScript('<script type="text/javascript" src="http://www.google.com/jsapi"></script>');
		$wgOut->addScript("<script type=\"text/javascript\">
      google.load(\"visualization\", \"1\", {packages:[\"piechart\"]});
      google.setOnLoadCallback(drawChart);
      function drawChart() {
        //Tell Google Visualization where your script is
        var query = new google.visualization.Query('/w/extensions/Helpmebot/vis.php');
        query.setQuery('select al_class, count(*) from hmb_accesslog group by al_class');
        query.send(function(result) {
          if(result.isError()) {
            alert(result.getDetailedMessage());
          } else {
            var chart = new google.visualization.PieChart(document.getElementById('chart_div'));
            chart.draw(result.getDataTable(), {width: 400, height: 240});
          }
        });
      }
    </script>");
		//$wgOut->addModules("ext.Helpmebot");
		$this->setHeaders();
		
		$wgOut->addHTML('<div id="chart_div"></div>');
	}
}
