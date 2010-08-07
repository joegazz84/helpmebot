<?php

class SpecialAccessList extends SpecialPage {
	function __construct() {
		parent::__construct( 'AccessList' );
		wfLoadExtensionMessages('Helpmebot');
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$out = "Access list is shown below. Table is scanned from top to bottom, first matching entry provides";
		$out.= " the access level.";
		
		$out.= '<p><span style="color:purple;">Developer</span><br />';
		$out.= '<span style="color:blue;">Superuser</span><br /> ';
		$out.= '<span style="color:green;"">Advanced</span><br /> ';
		$out.= '<span style="color:black;">Normal</span><br /> ';
		$out.= '<span style="color:orange;">Semi-ignored</span><br /> ';
		$out.= '<span style="color:red;">Ignored</span></p> ';
		
		$pager = new AccessListPager();
		$wgOut->addHTML( $out );
		$wgOut->addHTML( $pager->getNavigationBar() . '<table>' . $pager->getBody() . '</table>' . $pager->getNavigationBar() );
	}
}
