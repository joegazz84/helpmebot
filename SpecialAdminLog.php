<?php

class SpecialAdminLog extends SpecialPage {
	function __construct() {
		parent::__construct( 'AdminLog' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgUser, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );
			
		$pager = new AdminLogPager();
		$pager->setLimit(1000);
		$wgOut->addHTML( $out );
		$wgOut->addHTML( '<table>' . $pager->getBody() . '</table>'  );
	}
}
