<?php

class AccessListPager extends TablePager
{

	function getQueryInfo()
	{
		$queryInfo = array();
		
		
		$queryInfo['tables'] = 'hmb_user';
		$queryInfo['fields'] = array('user_id', 'user_nickname','user_username', 'user_hostname', 'user_accesslevel', 'sort');
		
		global $wgUser;
		if(! $wgUser->isAllowed('helpmebot-view-ignorelist'))
		{
			$queryInfo['conds'] = '`user_accesslevel` != "Ignored"';
		}
		
		return $queryInfo;
	}

	function getIndexField(){return "user_id";}

	function getRowClass($row){return "accesslistentry-" . $row->user_accesslevel; }

	function isFieldSortable( $field )
	{ return false; }

	function formatValue( $name, $value )
	{
		return $value;
	}

	function getDefaultSort()
	{
		return "sort"; //user_accesslevel";
	}

	function getFieldNames()
	{
		return array(
		'user_id'	=> "Access ID",
		'user_nickname' => "Nickname",
		'user_username' => "Username",
		'user_hostname' => "Hostname",
		'user_accesslevel' => "Access level"
		);
	}

}
