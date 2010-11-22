<?php
class SpecialCommandStatistics extends SpecialPage {
	function __construct() {
		parent::__construct( 'CommandStatistics' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		
		$wgOut->addModules(array("ext.GoogleJsapi", "ext.Helpmebot"));
		$this->setHeaders();
		
		$wgOut->addHTML('<div id="chart_div"></div>');
	}
}
