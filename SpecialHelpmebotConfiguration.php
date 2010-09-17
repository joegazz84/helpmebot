<?php

class SpecialHelpmebotConfiguration extends SpecialPage {
	function __construct() {
		parent::__construct( 'HelpmebotConfiguration' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$channel = $wgRequest->getIntOrNull('ircchannel');
		if($channel == null) {
			$wgOut->addWikiMsg('hmb-configuration-headertext');
					
			$pager = new ConfigChannelListPager();
			$wgOut->addHTML( $out );
			$wgOut->addHTML( '<table>' . $pager->getBody() . '</table>' );
		}
		else {
			$wgOut->addWikiMsg('hmb-configuration-channeltext');
		}
	}
}
