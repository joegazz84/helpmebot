<?php
class SpecialSubversionRepositories extends SpecialPage {
	function __construct() {
		parent::__construct( 'SubversionRepositories' );
	}
 
	function execute( $par ) {
		global $wgRequest, $wgOut, $wgUser, $wgScriptPath, $wgHelpmebotStyleVersion;
 
		$this->setHeaders();
		$wgOut->addExtensionStyle($wgScriptPath . '/extensions/Helpmebot/hmb.css?' . $wgHelpmebotStyleVersion );

		$folder = scandir('/var/lib/helpmebotsvn/');
		foreach( $folder as $item)
		{
		        if(is_dir('/var/lib/helpmebotsvn/' . $item))
		        {
		                if($item == "." || $item == ".." )
		                        continue;
		                $out .= "* [http://svn.helpmebot.org.uk/svn/" . $item . "/ " . $item."]\n";
		                
		        }
		}
		
		$wgOut->addWikiText($out,true);
	}
}