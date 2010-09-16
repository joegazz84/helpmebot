<?php

class ConfigurationPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'hmb_user',
			'fields' => array('user_id', 'user_nickname','user_username', 'user_hostname', 'user_accesslevel', 'sort')
			);
	}

	function getIndexField(){return "sort";}

	function getRowClass($row){return "accesslistentry-" . $row->user_accesslevel; }

	function isFieldSortable( $field )
	{ return false; }

	function formatValue( $name, $value )
	{
		return $value;
	}

	function getDefaultSort()
	{
		return ""; //user_accesslevel";
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
