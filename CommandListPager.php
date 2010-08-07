<?php

class AccessListPager extends TablePager
{

	function getQueryInfo()
	{
		return array(
			'tables' => 'helpmebot_v6.commands',
			'fields' => array('command','description', 'switches', 'accesslevel')
			);
	}

	function getIndexField(){return "command";}

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
		
		'command' => "Command",
		'description' => "Description",
		'switches' => "Switches",
		'accesslevel' => "Access Level"
		);
	}

}
