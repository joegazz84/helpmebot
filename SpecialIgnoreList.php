<?php

class SpecialIgnoreList extends SpecialPage {
	function __construct() {
		parent::__construct( 'IgnoreList', 'helpmebot-view-ignorelist' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$wgOut->addWikiMsg('hmb-ignorelist-headertext');
		
		$out = "";
		
		
		$out.= '<p><span style="color:purple;">'.wfMsg('hmb-developer').'</span><br />';
		$out.= '<span style="color:blue;">'.wfMsg('hmb-superuser').'</span><br /> ';
		$out.= '<span style="color:green;"">'.wfMsg('hmb-advanced').'</span><br /> ';
		$out.= '<span style="color:black;">'.wfMsg('hmb-normal').'</span><br /> ';
		$out.= '<span style="color:orange;">'.wfMsg('hmb-semi-ignored').'</span><br /> ';
		$out.= '<span style="color:red;">'.wfMsg('hmb-ignored').'</span></p> ';
		
		$pager = new IgnoreListPager();
		$wgOut->addHTML( $out );
		$wgOut->addHTML( $pager->getNavigationBar() . '<table>' . $pager->getBody() . '</table>' . $pager->getNavigationBar() );
	}
}
