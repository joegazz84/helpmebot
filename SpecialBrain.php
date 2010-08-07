<?php

class SpecialBrain extends SpecialPage {
	function __construct() {
		parent::__construct( 'Brain' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
//		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$out = "This is a list of all the words I recognise:";
		$pager = new BrainPager();
		$wgOut->addHTML( $out );
		$wgOut->addHTML( $pager->getNavigationBar() . '<table>' . $pager->getBody() . '</table>' . $pager->getNavigationBar() );
	}
}
