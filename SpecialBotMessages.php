<?php

class SpecialBotMessages extends SpecialPage {
	function __construct() {
		parent::__construct( 'BotMessages' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );
		
		$out = "Message list is shown below.";
//		$out.= " the access level.";
		
		$wgOut->addHTML( $out );
	}
}
