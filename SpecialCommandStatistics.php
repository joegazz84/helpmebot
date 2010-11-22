<?php
class SpecialCommandStatistics extends SpecialPage {
	function __construct() {
		parent::__construct( 'CommandStatistics' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		
		
		$wgOut->addScript('<script type="text/javascript" src="http://www.google.com/jsapi"></script>');
		$wgOut->addModules("ext.Helpmebot");
		$this->setHeaders();
		
		$wgOut->addHTML('<div id="chart_div"></div>');
	}
}
