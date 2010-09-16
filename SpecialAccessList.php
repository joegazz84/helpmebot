<?php

class SpecialAccessList extends SpecialPage {
	function __construct() {
		parent::__construct( 'AccessList' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgUser, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		if(! $wgUser->isAllowed('helpmebot-view-ignorelist'))
		{
			$wgOut->addWikiMsg('hmb-accesslist-headertext');
		}
		else 
		{
			$wgOut->addWikiMsg('hmb-ignorelist-headertext');
		}
		
		$out = "";
		
		$out.= '<p><span style="color:purple;">'.wfMsg('hmb-developer').'</span><br />';
		$out.= '<span style="color:blue;">'.wfMsg('hmb-superuser').'</span><br /> ';
		$out.= '<span style="color:green;"">'.wfMsg('hmb-advanced').'</span><br /> ';
		$out.= '<span style="color:black;">'.wfMsg('hmb-normal').'</span><br /> ';
		$out.= '<span style="color:orange;">'.wfMsg('hmb-semi-ignored').'</span><br /> ';
		
		
		if(! $wgUser->isAllowed('helpmebot-view-ignorelist'))
		{
			$out.= '<span style="color:red;">'.wfMsg('hmb-ignored-hidden').'</span></p> ';
		}
		else 
		{
			$out.= '<span style="color:red;">'.wfMsg('hmb-ignored').'</span></p> ';
		}
		
		$pager = new AccessListPager();
		$pager->setLimit(1000);
		$wgOut->addHTML( $out );
		$wgOut->addHTML( '<table>' . $pager->getBody() . '</table>'  );
	}
}
