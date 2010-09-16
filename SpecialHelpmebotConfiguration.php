<?php

class SpecialHelpmebotConfiguration extends SpecialPage {
	function __construct() {
		parent::__construct( 'HelpmebotConfiguration' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$wgOut->addWikiMsg('hmb-configuration-headertext');
		
		$out = "";
		
		$pager = new ConfigChannelListPager();
		$wgOut->addHTML( $out );
		$wgOut->addHTML( $pager->getNavigationBar() . '<table>' . $pager->getBody() . '</table>' . $pager->getNavigationBar() );
	}
}
