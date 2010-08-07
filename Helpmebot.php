<?php

#ini_set('display_errors',1);

if(!defined('MEDIAWIKI')) {
	echo("This file is an extension to the mediawiki software and cannot be used standalone\n");
	die( 1 );
}

$wgExtensionCredits['specialpage'][] = array(
'path' => __FILE__,
'name' => 'Helpmebot Viewer',
'description' => 'Allows viewing of Helpmebot\'s configuration etc.',
'author' => 'Simon Walker'
);

SpecialVersion::$viewvcUrls['http://svn.helpmebot.org.uk/svn/helpmebot'] = 'http://svn.helpmebot.org.uk/viewvc/helpmebot';

$wgAutoloadClasses['SpecialAccessList'] = $IP . '/extensions/Helpmebot/SpecialAccessList.php';
$wgAutoloadClasses['AccessListPager'] = $IP . '/extensions/Helpmebot/AccessListPager.php';
$wgAutoloadClasses['SpecialBrain'] = $IP.'/extensions/Helpmebot/SpecialBrain.php';
$wgAutoloadClasses['BrainPager'] = $IP.'/extensions/Helpmebot/BrainPager.php';
//$wgAutoloadClasses['CommandListPager'] = $IP.'/extensions/Helpmebot/CommandListPager.php';
$wgAutoloadClasses['SpecialCommandList'] = $IP . '/extensions/Helpmebot/SpecialCommandList.php';

$wgSpecialPages['AccessList'] = 'SpecialAccessList';
$wgSpecialPages['Brain'] = 'SpecialBrain';
$wgSpecialPages['CommandList'] = 'SpecialCommandList';

$wgSpecialPageGroups['AccessList'] = 'helpmebot';
$wgSpecialPageGroups['Brain']="helpmebot";
$wgSpecialPageGroups['CommandList']="helpmebot";

$wgExtensionMessagesFiles['Helpmebot'] = $IP . '/extensions/Helpmebot/Helpmebot.i18n.php';

$wgHelpmebotStyleVersion=2;
